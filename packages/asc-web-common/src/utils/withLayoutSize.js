import * as React from "react";
import { Consumer } from "@appserver/components/src/utils/context";

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
