import { NavBars } from "./NavBars";
import { configuration } from "./index";
/**
 * App component is return only the navbars
 * To be extended in the future if needed
 * @returns
 */
function App() {
  return <NavBars {...configuration} />;
}

export default App;
