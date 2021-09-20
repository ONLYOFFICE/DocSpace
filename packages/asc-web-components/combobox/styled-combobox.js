import styled from "styled-components";
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

  .dropdown-container {
    padding: ${(props) =>
      props.advancedOptions && props.theme.comboBox.padding};
  }
  -webkit-user-select: none;
`;

StyledComboBox.defaultProps = {
  theme: Base,
};

export default StyledComboBox;
