import styled from "styled-components";
import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";

const CHIPS_BORDER_RADIUS = "3px",
  CHIPS_BACKGROUND = "#ECEEF1",
  CHIPS_COLOR = "#333333";

const StyledContent = styled.div`
  width: 469px;
  height: 88px;
`;

const StyledChip = styled.div`
  width: fit-content;

  display: flex;
  align-items: center;
  justify-content: center;

  box-sizing: border-box;
  background: ${CHIPS_BACKGROUND};

  height: 20px;
  margin: 4px;
  padding: 2px 4px;

  border-radius: ${CHIPS_BORDER_RADIUS} 0 0 ${CHIPS_BORDER_RADIUS};
  border: ${(props) => props.isSelected && "1px dashed #000"};
`;

const StyledChipValue = styled.div`
  margin-right: 4px;

  font-weight: normal;
  font-size: 14px;

  color: ${CHIPS_COLOR};
`;

const StyledChipGroup = styled.div`
  height: fit-content;
  ${commonInputStyle} :focus-within {
    border-color: ${(props) => props.theme.inputBlock.borderColor};
  }

  .scroll {
    position: inherit !important;
    display: flex;
    flex-wrap: wrap;
  }

  input {
    flex: 1 0 auto;
  }
`;
StyledChipGroup.defaultProps = { theme: Base };

const StyledChipWithInput = styled.div`
  min-height: 32px;
  max-height: 88px;
  display: flex;
  flex-wrap: wrap;
  height: fit-content;
`;

export {
  StyledContent,
  StyledChip,
  StyledChipValue,
  StyledChipGroup,
  StyledChipWithInput,
};
