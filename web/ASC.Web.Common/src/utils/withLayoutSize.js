import * as React from "react";
import { utils } from "asc-web-components";
const { Consumer } = utils.context;

export function withLayoutSize(Component) {
  return function LayoutSizeComponent(props) {
    return (
      <Consumer>
        {(context) => {
          return <Component {...props} {...context} />;
        }}
      </Consumer>
    );
  };
}
