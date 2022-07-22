import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
//import ReactDOMServer from "react-dom/server";
//import { Parser } from "html-to-react";

const iconSizes = {
  small: 12,
  medium: 16,
  big: 24,
};

const getSizeStyle = (size) => {
  switch (size) {
    case "scale":
      return `
        &:not(:root) {
          width: 100%;
          height: 100%;
        }
      `;
    case "small":
    case "medium":
    case "big":
      return `
        width: ${iconSizes[size]}px;
        min-width: ${iconSizes[size]}px;
        height: ${iconSizes[size]}px;
        min-height: ${iconSizes[size]}px;
      `;
    default:
      return `
        width: ${iconSizes.big}px;
        min-width: ${iconSizes.big}px;
        height: ${iconSizes.big}px;
        min-height: ${iconSizes.big}px;
      `;
  }
};

export default function createStyledIcon(
  Component,
  displayName,
  fillPath = "*",
  strokePath = "*"
) {
  class Icon extends React.Component {
    render_xml(id, xml_string) {
      var doc = new DOMParser().parseFromString(xml_string, "application/xml");
      var el = document.getElementById(id);
      el.appendChild(el.ownerDocument.importNode(doc.documentElement, true));
    }

    render() {
      // eslint-disable-next-line react/prop-types, no-unused-vars
      const {
        isfill,
        isStroke,
        color,
        stroke,
        fillPath,
        strokePath,
        ...props
      } = this.props;

      // let svg = ReactDOMServer.renderToString(
      //   <Component {...props}></Component>
      // );
      // const matchResult = svg.match(/\s*mask id="(\w*)"\s/);
      // const newId =
      //   Math.random().toString(36).substring(2, 5) +
      //   Math.random().toString(36).substring(2, 5);

      // if (matchResult != null) {
      //   if (matchResult.length > 1) {
      //     svg = svg.replace(new RegExp(matchResult[1], "g"), newId);

      //     const htmlToReactParser = new Parser();
      //     const reactComponent = htmlToReactParser.parse(svg);
      //     return reactComponent;
      //   }
      // }
      return <Component {...props}></Component>;
    }
  }

  const StyledIcon = styled(Icon)(
    (props) => `
    ${props.fillPath} {
      ${props.isfill ? "fill:" + props.color : ""};
    }
    ${props.strokePath} {
      ${props.isStroke ? "stroke:" + props.stroke : ""};
    }
    overflow: hidden;
    vertical-align: middle;
    ${getSizeStyle(props.size)}
  `
  );

  StyledIcon.displayName = displayName;

  StyledIcon.propTypes = {
    color: PropTypes.string,
    stroke: PropTypes.string,
    size: PropTypes.oneOf(["small", "medium", "big", "scale"]),
    isfill: PropTypes.bool,
    isStroke: PropTypes.bool,
    fillPath: PropTypes.string,
    strokePath: PropTypes.string,
  };

  StyledIcon.defaultProps = {
    fillPath: fillPath,
    strokePath: strokePath,
  };

  return StyledIcon;
}
