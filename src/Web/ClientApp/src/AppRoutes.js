import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { NumberAdder } from "./components/NumberAdder";

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
  }
];

export default AppRoutes;
