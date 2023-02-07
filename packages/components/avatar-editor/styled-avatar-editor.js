import styled from "styled-components";
import { Base } from "../themes";

const StyledButtonsWrapper = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-gap: 8px;
  min-width: ${(props) => props.theme.avatarEditor.minWidth};
  max-width: ${(props) => props.theme.avatarEditor.maxWidth};
  width: ${(props) => props.theme.avatarEditor.width};
`;
StyledButtonsWrapper.defaultProps = { theme: Base };

export default StyledButtonsWrapper;
