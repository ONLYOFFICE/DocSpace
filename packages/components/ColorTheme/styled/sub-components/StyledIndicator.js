import styled from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledIndicator = styled.div`
  border-radius: 50%;
  width: 8px;
  height: 8px;
  background: ${(props) => props.theme.filterInput.filter.indicatorColor};
  position: absolute;
  top: 25px;
  left: 25px;

  z-index: 10;
`;

StyledIndicator.defaultProps = { theme: Base };

export default StyledIndicator;
