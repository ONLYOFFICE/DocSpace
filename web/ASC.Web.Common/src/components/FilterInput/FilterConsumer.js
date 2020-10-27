import React from "react";
import { utils } from "asc-web-components";
import FilterInput from "./FilterInput";

const { Consumer } = utils.context;

export const FilterConsumer = (props) => (
  <Consumer>
    {(context) => <FilterInput widthProp={context.sectionWidth} {...props} />}
  </Consumer>
);
