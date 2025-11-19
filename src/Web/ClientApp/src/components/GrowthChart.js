import React, { useEffect, useState } from 'react';
import { Card, Title, Loader, Text, Group, Select, Button } from '@mantine/core';
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

function generateYearOptions() {
    const currentYear = new Date().getFullYear();
    const options = [];
    
    for (let year = currentYear - 10; year <= currentYear; year++) {
        options.push({
            value: year.toString(),
            label: year.toString()
        });
    }
    
    return options;
}

function generateQuarterOptions() {
    return [
        { value: '1', label: 'Q1' },
        { value: '2', label: 'Q2' },
        { value: '3', label: 'Q3' },
        { value: '4', label: 'Q4' }
    ];
}

function quarterToDate(year, quarter) {
    if (!year || !quarter) return null;
    const q = parseInt(quarter);
    const month = (q - 1) * 3 + 1;
    return `${year}-${String(month).padStart(2, '0')}-01`;
}

function getQuarterEndDate(year, quarter) {
    if (!year || !quarter) return null;
    const q = parseInt(quarter);
    const lastMonth = q * 3;
    const lastDay = new Date(parseInt(year), lastMonth, 0).getDate();
    return `${year}-${String(lastMonth).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;
}

function getDefaultFromYear() {
    return (new Date().getFullYear() - 4).toString();
}

function getDefaultFromQuarter() {
    return '1';
}

function getDefaultToYear() {
    return new Date().getFullYear().toString();
}

function getDefaultToQuarter() {
    const currentMonth = new Date().getMonth();
    const currentQuarter = Math.floor(currentMonth / 3) + 1;
    return currentQuarter.toString();
}

export default function GrowthChart() {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const yearOptions = generateYearOptions();
    const quarterOptions = generateQuarterOptions();
    
    const [fromYear, setFromYear] = useState(getDefaultFromYear());
    const [fromQuarter, setFromQuarter] = useState(getDefaultFromQuarter());
    const [toYear, setToYear] = useState(getDefaultToYear());
    const [toQuarter, setToQuarter] = useState(getDefaultToQuarter());

    const loadData = async () => {
        setLoading(true);
        try {
            const params = new URLSearchParams();
            
            const dateFrom = quarterToDate(fromYear, fromQuarter);
            if (dateFrom) params.append('dateFrom', dateFrom);
            
            const dateTo = getQuarterEndDate(toYear, toQuarter);
            if (dateTo) params.append('dateTo', dateTo);

            const queryString = params.toString();
            const cowUrl = `/api/Cows/per-quarter${queryString ? '?' + queryString : ''}`;
            const chickenUrl = `/api/Chickens/per-quarter${queryString ? '?' + queryString : ''}`;

            const [cRes, hRes] = await Promise.all([
                fetch(cowUrl),
                fetch(chickenUrl),
            ]);

            const [cowData, chickenData] = await Promise.all([
                cRes.ok ? cRes.json() : [],
                hRes.ok ? hRes.json() : [],
            ]);

            // Merge data by quarter
            const quarterMap = new Map();

            cowData.forEach(item => {
                quarterMap.set(item.quarter, { quarter: item.quarter, cows: item.count, chickens: 0 });
            });

            chickenData.forEach(item => {
                if (quarterMap.has(item.quarter)) {
                    quarterMap.get(item.quarter).chickens = item.count;
                } else {
                    quarterMap.set(item.quarter, { quarter: item.quarter, cows: 0, chickens: item.count });
                }
            });

            const chartData = Array.from(quarterMap.values());

            setData(chartData);
        } catch (err) {
            console.error(err);
            setData([]);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleApplyFilters = () => {
        loadData();
    };

    const handleClearFilters = () => {
        setFromYear(getDefaultFromYear());
        setFromQuarter(getDefaultFromQuarter());
        setToYear(getDefaultToYear());
        setToQuarter(getDefaultToQuarter());
    };

    return (
        <div>
            <Card shadow="sm" padding="lg" radius="md" style={{ marginBottom: '1rem' }}>
                <Title order={5} style={{ marginBottom: 12 }}>Filters</Title>
                <Group>
                    <Select
                        label="From Year"
                        value={fromYear}
                        onChange={setFromYear}
                        data={yearOptions}
                        style={{ width: 100 }}
                    />
                    <Select
                        label="From Quarter"
                        value={fromQuarter}
                        onChange={setFromQuarter}
                        data={quarterOptions}
                        style={{ width: 100 }}
                    />
                    <Select
                        label="To Year"
                        value={toYear}
                        onChange={setToYear}
                        data={yearOptions}
                        style={{ width: 100 }}
                    />
                    <Select
                        label="To Quarter"
                        value={toQuarter}
                        onChange={setToQuarter}
                        data={quarterOptions}
                        style={{ width: 100 }}
                    />
                    <Button onClick={handleApplyFilters} style={{ marginTop: '1.5rem' }}>
                        Apply
                    </Button>
                    <Button onClick={handleClearFilters} variant="outline" style={{ marginTop: '1.5rem' }}>
                        Reset
                    </Button>
                </Group>
            </Card>

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
        </div>
    );
}