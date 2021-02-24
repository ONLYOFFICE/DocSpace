import styled from "styled-components";
import { Base } from "../../themes";

const StyledSelectedItem = styled.div`
  position: relative;
  display: ${(props) => (props.isInline ? "inline-grid" : "grid")};
  grid-template-columns: 1fr auto;
  box-sizing: border-box;
  background: ${(props) => props.theme.selectedItem.background};
  border: ${(props) => props.theme.selectedItem.border};
  border-radius: ${(props) => props.theme.selectedItem.borderRadius};
`;
StyledSelectedItem.defaultProps = { theme: Base };

const StyledSelectedTextBox = styled.div`
  display: grid;
  padding: ${(props) => props.theme.selectedItem.textBox.padding};
  height: ${(props) => props.theme.selectedItem.textBox.height};
  align-items: ${(props) => props.theme.selectedItem.textBox.alignItems};
  border-right: ${(props) => props.theme.selectedItem.textBox.borderRight};
  cursor: default;
`;
StyledSelectedTextBox.defaultProps = { theme: Base };

const StyledCloseButton = styled.div`
  display: flex;
  align-items: ${(props) => props.theme.selectedItem.closeButton.alignItems};
  padding: ${(props) => props.theme.selectedItem.closeButton.padding};
  cursor: ${(props) => (!props.isDisabled ? "pointer" : "default")};

  &:hover {
    path {
      ${(props) =>
        !props.isDisabled &&
        `fill: ${props.theme.selectedItem.closeButton.colorHover};`}
    }
  }

  &:active {
    ${(props) =>
      !props.isDisabled &&
      `background-color: ${props.theme.selectedItem.closeButton.backgroundColor};`}
  }
`;
StyledCloseButton.defaultProps = { theme: Base };

export { StyledCloseButton, StyledSelectedTextBox, StyledSelectedItem };
