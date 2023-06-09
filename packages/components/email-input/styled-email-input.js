import styled from "styled-components";
import StyledTextInput from "../text-input/styled-text-input";

const StyledEmailInput = styled(StyledTextInput)`
  :placeholder-shown {
    direction: ltr;
  }
`;

export default StyledEmailInput;
