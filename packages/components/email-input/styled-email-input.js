import styled from "styled-components";
import StyledTextInput from "../text-input/styled-text-input";

const StyledEmailInput = styled(StyledTextInput)`
  text-align: ${({ theme }) =>
    theme.interfaceDirection === "rtl" ? "right" : "left"};
`;

export default StyledEmailInput;
