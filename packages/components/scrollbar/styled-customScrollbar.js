import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledScrollbar = styled.div`
  position: relative;
  overflow: hidden;
  width: 100%;
  height: 100%;

  .container {
    position: absolute;
    inset: 0;
    width: ${(props) => props.width && props.width + "px"};
    height: ${(props) => props.height && props.height + "px"};

    overflow: scroll;
    box-sizing: border-box;
    scrollbar-face-color: ${(props) =>
      props.theme.scrollbar.hoverBackgroundColorVertical};
    scrollbar-track-color: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorVertical};
    scrollbar-color: ${(props) =>
        props.theme.scrollbar.hoverBackgroundColorVertical}
      transparent;
    scrollbar-width: thin;
  }
  .container::-webkit-scrollbar-corner {
    background: transparent;
  }
  .container::-webkit-scrollbar {
    width: ${(props) => props.thumbv?.width};
    height: ${(props) => (props.stype === "thumbV" ? 0 : props.thumbh?.height)};
  }

  .container::-webkit-scrollbar-track {
    background: transparent;
    border-radius: inherit;
  }

  .container::-webkit-scrollbar-thumb {
    background: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorVertical};
    border-radius: inherit;
  }
  .container::-webkit-scrollbar-thumb:hover {
    background: ${(props) =>
      props.theme.scrollbar.hoverBackgroundColorVertical};
    border-radius: inherit;
  }

  .container::-webkit-scrollbar-thumb:active {
    background: ${(props) =>
      props.theme.scrollbar.hoverBackgroundColorVertical};
    border-radius: inherit;
  }
`;

StyledScrollbar.defaultProps = {
  theme: Base,
};

export default StyledScrollbar;
