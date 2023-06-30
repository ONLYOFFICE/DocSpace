import styled from "styled-components";

import Combobox from "../../../combobox";
import Text from "../../../text";
import Base from "../../../themes/base";

const StyledFooter = styled.div<{
  withFooterInput?: boolean;
  withFooterCheckbox?: boolean;
}>`
  width: calc(100% - 32px);
  max-height: ${(props) =>
    props.withFooterCheckbox
      ? "181px"
      : props.withFooterInput
      ? "145px"
      : "73px"};
  height: ${(props) =>
    props.withFooterCheckbox
      ? "181px"
      : props.withFooterInput
      ? "145px"
      : "73px"};
  min-height: ${(props) =>
    props.withFooterCheckbox
      ? "181px"
      : props.withFooterInput
      ? "145px"
      : "73px"};

  padding: 0 16px;

  border-top: ${(props) => props.theme.selector.border};

  .button {
    min-height: 40px;

    margin-bottom: 2px;
  }
`;

StyledFooter.defaultProps = { theme: Base };

const StyledNewNameContainer = styled.div`
  margin-top: 16px;

  .new-file-input {
    margin-bottom: 16px;
  }
`;

const StyledNewNameHeader = styled(Text)`
  margin-bottom: 4px;
`;

const StyledButtonContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;

  gap: 8px;

  margin-top: 16px;
`;

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

export {
  StyledFooter,
  StyledNewNameContainer,
  StyledNewNameHeader,
  StyledButtonContainer,
  StyledComboBox,
};
