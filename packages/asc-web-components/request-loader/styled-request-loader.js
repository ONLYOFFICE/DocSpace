import styled from "styled-components";
import Loader from "../loader";
import Base from "../themes/base";

const StyledOuter = styled.div`
  position: fixed;
  text-align: center;
  top: ${(props) => props.theme.requestLoader.top};
  width: ${(props) => props.theme.requestLoader.widths};
  z-index: ${(props) => props.zIndex};
  display: ${(props) => (props.visible ? "block" : "none")};
`;
StyledOuter.defaultProps = { theme: Base };

const StyledInner = styled.div`
  background-color: ${(props) => props.theme.requestLoader.backgroundColor};
  border: ${(props) => props.theme.requestLoader.border};

  display: inline-block;
  white-space: nowrap;

  overflow: ${(props) => props.theme.requestLoader.overflow};
  padding: ${(props) => props.theme.requestLoader.padding};
  line-height: ${(props) => props.theme.requestLoader.lineHeight};
  z-index: ${(props) => props.zIndex};
  border-radius: ${(props) => props.theme.requestLoader.borderRadius};
  -moz-border-radius: ${(props) => props.theme.requestLoader.borderRadius};
  -webkit-border-radius: ${(props) => props.theme.requestLoader.borderRadius};
  box-shadow: ${(props) => props.theme.requestLoader.boxShadow};
  -moz-box-shadow: ${(props) => props.theme.requestLoader.boxShadow};
  -webkit-box-shadow: ${(props) => props.theme.requestLoader.boxShadow};

  .text-style {
    display: contents;
  }
`;
StyledInner.defaultProps = { theme: Base };

const OvalLoader = styled(Loader)`
  display: inline;
  margin-right: ${(props) => props.theme.requestLoader.marginRight};
  svg {
    vertical-align: middle;
  }
`;
OvalLoader.defaultProps = { theme: Base };

export { OvalLoader, StyledInner, StyledOuter };
