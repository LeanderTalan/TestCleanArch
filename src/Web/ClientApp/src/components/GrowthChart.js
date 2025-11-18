import React, { useEffect, useState } from 'react';
import { Card, Title, Loader, Text } from '@mantine/core';
import {
  ResponsiveContainer,
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  Legend,
  CartesianGrid,
} from 'recharts';

function parseDate(val) {
  if (!val) return null;
  // Accept "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ssZ", etc
  const d = new Date(val);
  return Number.isNaN(d.getTime()) ? null : d;
}

function quarterOf(date) {
  const q = Math.floor(date.getMonth() / 3) + 1;
  return { year: date.getFullYear(), quarter: q };
}

function quarterLabel(year, quarter) {
  return `${year} Q${quarter}`;
}

export default function GrowthChart() {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      setLoading(true);
      try {
        const [cRes, hRes] = await Promise.all([
          fetch('/api/Cows'),
          fetch('/api/Chickens'),
        ]);

        const [cows, chickens] = await Promise.all([
          cRes.ok ? cRes.json() : [],
          hRes.ok ? hRes.json() : [],
        ]);

        const cowDates = (cows || []).map((c) => parseDate(c.birthDate ?? c.BirthDate));
        const chickenDates = (chickens || []).map((c) => parseDate(c.birthDate ?? c.BirthDate));

        const allDates = [...cowDates, ...chickenDates].filter(Boolean);
        if (allDates.length === 0) {
          setData([]);
          setLoading(false);
          return;
        }

        // Find start quarter (min date) and end quarter (today)
        const minDate = new Date(Math.min(...allDates.map((d) => d.getTime())));
        const maxDate = new Date(); // to current quarter
        let { year: startY, quarter: startQ } = quarterOf(minDate);
        const { year: endY, quarter: endQ } = quarterOf(maxDate);

        // Build quarter list
        const quarters = [];
        let y = startY, q = startQ;
        while (y < endY || (y === endY && q <= endQ)) {
          quarters.push({ year: y, quarter: q });
          q++;
          if (q > 4) {
            q = 1;
            y++;
          }
        }

        // For each quarter compute cumulative counts up to end of quarter
        const chartData = quarters.map(({ year, quarter }) => {
          // quarter end date: (month = quarter*3) - day 0 -> last day prev month
          const monthEnd = quarter * 3;
          const quarterEndDate = new Date(year, monthEnd, 0, 23, 59, 59, 999);

          const cowsCount = cowDates.filter((d) => d && d.getTime() <= quarterEndDate.getTime()).length;
          const chickensCount = chickenDates.filter((d) => d && d.getTime() <= quarterEndDate.getTime()).length;

          return { quarter: quarterLabel(year, quarter), cows: cowsCount, chickens: chickensCount };
        });

        setData(chartData);
      } catch (err) {
        console.error(err);
        setData([]);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, []);

  return (
    <Card shadow="sm" padding="lg" radius="md">
      <Title order={4} style={{ marginBottom: 12 }}>Animal growth per quarter</Title>

      {loading && <Loader />}

      {!loading && (!data || data.length === 0) && (
        <Text>No data available</Text>
      )}

      {!loading && data && data.length > 0 && (
        <div style={{ width: '100%', height: 360 }}>
          <ResponsiveContainer>
            <LineChart data={data}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="quarter" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Legend />
              <Line type="monotone" dataKey="cows" stroke="#4dabf7" name="Cows" />
              <Line type="monotone" dataKey="chickens" stroke="#74c69d" name="Chickens" />
            </LineChart>
          </ResponsiveContainer>
        </div>
      )}
    </Card>
  );
}