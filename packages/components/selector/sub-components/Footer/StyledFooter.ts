import styled from "styled-components";

import Combobox from "../../../combobox";
import Base from "../../../themes/base";

const StyledFooter = styled.div`
  width: calc(100% - 32px);
  max-height: 73px;
  height: 73px;
  min-height: 73px;

  padding: 0 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  gap: 8px;

  border-top: ${(props) => props.theme.selector.border};

  .button {
    min-height: 40px;

    margin-bottom: 2px;
  }
`;

StyledFooter.defaultProps = { theme: Base };

const StyledComboBox = styled(Combobox)`
  margin-bottom: 2px;
  max-height: 50px;

  .combo-button {
    min-height: 40px;
  }

  .combo-button-label,
  .combo-button-label:hover {
    font-size: 14px;
    text-decoration: none;
  }
`;

export { StyledFooter, StyledComboBox };
