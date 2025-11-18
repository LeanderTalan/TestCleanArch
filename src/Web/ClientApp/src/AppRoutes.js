import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { NumberAdder } from "./components/NumberAdder";
import { AnimalAdder } from "./components/AnimalAdder";
import GrowthChart from './components/GrowthChart';

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <><Counter /><NumberAdder /></>
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: "/animal-adder",
    element: <AnimalAdder />
  },
  {
    path: '/growth',
    element: <GrowthChart />
  }
];

export default AppRoutes;
