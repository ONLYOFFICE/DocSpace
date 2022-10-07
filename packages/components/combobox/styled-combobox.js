import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledComboBox = styled.div`
  width: ${(props) =>
    (props.scaled && "100%") ||
    (props.size === "base" && props.theme.comboBox.width.base) ||
    (props.size === "middle" && props.theme.comboBox.width.middle) ||
    (props.size === "big" && props.theme.comboBox.width.big) ||
    (props.size === "huge" && props.theme.comboBox.width.huge) ||
    (props.size === "content" && "fit-content")};

  position: relative;
  outline: 0;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  padding: 4px 0;

  ${(props) =>
    props.isOpen &&
    props.noBorder &&
    css`
      background: ${(props) => props.theme.comboBox.background};
      border-radius: 3px;
    `}

  .dropdown-container {
    padding: ${(props) =>
      props.advancedOptions && props.theme.comboBox.padding};

    @media (max-width: 428px) {
      position: fixed;
      top: unset;
      right: 0;
      left: 0;
      bottom: 0;
      width: 100%;
      width: -moz-available;
      width: -webkit-fill-available;
      width: fill-available;
      border: none;
      border-radius: 6px 6px 0px 0px;
    }
  }
  -webkit-user-select: none;

  .backdrop-active {
    z-index: 210;
  }
`;

StyledComboBox.defaultProps = {
  theme: Base,
};

export default StyledComboBox;
