import styled from "styled-components";
import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";
import TextInput from "../text-input";

const StyledChipWithInput = styled.div`
  min-height: 32px;
  max-height: 220px;
  width: 100%;
  display: flex;
  flex-wrap: wrap;
  height: fit-content;
  cursor: auto;
  width: ${(props) => props.length === 0 && "100%"};
`;

const StyledContent = styled.div`
  position: relative;
  width: 469px;
  height: 220px;
`;

const StyledChipGroup = styled.div`
  :focus-visible {
    outline: 0px solid #2da7db !important;
  }
  height: fit-content;
  ${commonInputStyle} :focus-within {
    border-color: ${(props) => props.theme.inputBlock.borderColor};
  }

  .scroll {
    height: fit-content;
    position: inherit !important;
    display: flex;
    flex-wrap: wrap;

    :focus-visible {
      outline: 0px solid #2da7db !important;
    }
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
  background: #eceef1;

  height: 32px;
  margin: 2px 4px;
  padding: 5px 7px;

  border-radius: 3px 0 0 3px;
  background: ${(props) =>
    props.isValid ? props.theme.selectedItem.background : "#F7CDBE"};
  border: ${(props) =>
    props.isSelected
      ? props.theme.emailChips.dashedBorder
      : props.theme.selectedItem.border};

  user-select: none;

  .warning_icon_wrap {
    cursor: pointer;
    .warning_icon {
      margin-right: 4px;
    }
  }
`;
StyledChip.defaultProps = { theme: Base };

const StyledChipValue = styled.div`
  margin-right: 8px;
  min-width: 0px;
  max-width: 395px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  font-weight: normal;
  font-size: 13px;

  color: ${(props) =>
    props.isValid ? props.theme.selectedItem.text.color : "#333"};

  :hover {
    cursor: pointer;
  }
`;

const StyledContainer = styled.div`
  position: relative;
  height: 32px;
  margin: 3px 4px;
`;

const StyledChipInput = styled(TextInput)`
  flex: ${(props) => `${props.flexvalue}!important`};
`;

const StyledInputWithLink = styled.div`
  position: relative;

  display: grid;
  gap: 8px;
  grid-template-columns: auto 15%;
  align-content: space-between;
  width: calc(100% - 8px);

  .textInput {
    width: calc(100% - 8px);
    padding: 0px;
    margin: 8px 0px 10px 8px;
  }

  .link {
    text-align: end;
    margin: 10px 0px;
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
    margin-right: 8px;
  }
`;

const StyledTooltip = styled.div`
  position: absolute;
  top: -49px;
  left: 0;

  max-width: 435px;
  padding: 16px;

  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;

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
  StyledContainer,
  StyledChipInput,
  StyledInputWithLink,
  StyledTooltip,
};
