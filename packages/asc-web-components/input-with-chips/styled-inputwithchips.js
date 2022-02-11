import styled from "styled-components";
import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";
import TextInput from "../text-input";

const CHIPS_BORDER_RADIUS = "3px",
  CHIPS_BACKGROUND = "#ECEEF1",
  CHIPS_COLOR = "#333333";

const StyledChipWithInput = styled.div`
  min-height: 32px;
  max-height: 220px;
  width: 100%;
  display: flex;
  flex-wrap: wrap;
  height: fit-content;
  width: ${(props) => props.length === 0 && "100%"};
`;

const StyledContent = styled.div`
  width: 469px;
  height: 220px;
`;

const StyledChipGroup = styled.div`
  height: fit-content;
  ${commonInputStyle} :focus-within {
    border-color: ${(props) => props.theme.inputBlock.borderColor};
  }

  .scroll {
    height: fit-content;
    position: inherit !important;
    display: flex;
    flex-wrap: wrap;
  }

  input {
    flex: 1 0 auto;
  }
`;
StyledChipGroup.defaultProps = { theme: Base };

const StyledAllChips = styled.div`
  width: 448px;
  max-height: 180px;
  display: flex;
  flex-wrap: wrap;
  flex: 1 1 auto;
`;

const StyledChip = styled.div`
  width: fit-content;
  max-width: calc(100% - 18px);

  display: flex;
  align-items: center;
  justify-content: center;

  box-sizing: border-box;
  background: ${CHIPS_BACKGROUND};

  height: 32px;
  margin: 2px 4px;
  padding: ${(props) => (props.isSelected ? "5px 7px" : "6px 8px")};

  border-radius: ${CHIPS_BORDER_RADIUS} 0 0 ${CHIPS_BORDER_RADIUS};
  border: ${(props) => props.isSelected && "1px dashed #000"};
  background: ${(props) => (props.isValid ? "#ECEEF1" : "#F7CDBE")};

  .warning_icon_wrap {
    cursor: pointer;
    .warning_icon {
      margin-right: 4px;
    }
  }
`;

const StyledChipValue = styled.div`
  margin-right: 4px;
  min-width: 0px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  font-weight: normal;
  font-size: 13px;

  color: ${CHIPS_COLOR};
`;

const StyledChipInput = styled(TextInput)`
  flex: 0 1 auto !important;
`;

const StyledInputWithLink = styled.div`
  position: relative;
  width: 100%;
  display: flex;
  align-items: center;
`;

const StyledTooltip = styled.div`
  position: absolute;
  top: -49px;
  left: 0;
  width: 300px;
  padding: 16px;
  background: #f8f7bf;
  border-radius: 6px;
  opacity: 0.9;
`;

export {
  StyledChipWithInput,
  StyledContent,
  StyledChipGroup,
  StyledAllChips,
  StyledChip,
  StyledChipValue,
  StyledChipInput,
  StyledInputWithLink,
  StyledTooltip,
};
