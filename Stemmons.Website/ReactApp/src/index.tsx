import React from "react";
import ReactDOM from "react-dom";
import App from "./App";
import { IAppConfig } from "./IAppConfig";

export const configuration: IAppConfig = (window as any)["ReactMenuConfig"];

ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById(configuration.menuDivID)
);
