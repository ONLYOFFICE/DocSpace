import styled from "styled-components";
import Base from "../../themes/base";

export const ArrowIcon = styled.span`
  position: absolute;
  border-left: 2px solid;
  border-bottom: 2px solid;
  width: 5px;
  height: 5px;
`;

ArrowIcon.defaultProps = { theme: Base };
