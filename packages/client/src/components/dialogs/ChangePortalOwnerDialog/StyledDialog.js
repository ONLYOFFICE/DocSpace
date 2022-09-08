import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledOwnerInfo = styled.div`
  display: flex;
  align-items: center;
  justify-content: start;

  margin-top: 8px;
  margin-bottom: 24px;

  .info {
    padding-left: 16px;
    display: flex;
    flex-direction: column;

    .display-name {
      font-weight: 700;
      font-size: 16px;
      line-height: 22px;
    }

    .owner {
      font-weight: 600;
      font-size: 13px;
      line-height: 20px;
      color: ${(props) => props.theme.text.disableColor};
    }
  }
`;

StyledOwnerInfo.defaultProps = { theme: Base };

const StyledPeopleSelectorInfo = styled.div`
  margin-bottom: 12px;

  .new-owner {
    font-weight: 600;
    font-size: 15px;
    line-height: 16px;
    margin-bottom: 4px;
  }

  .description {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;

    color: ${(props) => props.theme.settings.owner.departmentColor};
  }
`;

StyledPeopleSelectorInfo.defaultProps = { theme: Base };

const StyledPeopleSelector = styled.div`
  display: flex;
  align-items: center;

  margin-bottom: 24px;

  .label {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;

    color: ${(props) => props.theme.settings.owner.departmentColor};

    margin-left: 8px;
  }
`;

StyledPeopleSelector.defaultProps = { theme: Base };

const StyledAvailableList = styled.div`
  display: flex;

  flex-direction: column;

  margin-bottom: 24px;

  .list-header {
    font-weight: 600;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 8px;
  }

  .list-item {
    font-weight: 400;
    font-size: 13px;
    line-height: 20px;

    margin-bottom: 2px;
  }
`;

StyledAvailableList.defaultProps = { theme: Base };

const StyledFooterWrapper = styled.div`
  height: 100%;
  width: 100%;

  display: flex;

  flex-direction: column;

  .info {
    margin-bottom: 16px;

    font-weight: 400;
    font-size: 13px;
    line-height: 20px;
  }

  .button-wrapper {
    display: flex;
    align-items: center;
    justify-content: space-between;

    flex-direction: row;

    gap: 8px;
  }
`;

StyledFooterWrapper.defaultProps = { theme: Base };

const StyledSelectedOwnerContainer = styled.div`
  width: 100%;

  box-sizing: border-box;

  display: flex;

  flex-direction: column;

  gap: 14px;

  margin-bottom: 24px;
`;

StyledSelectedOwnerContainer.defaultProps = { theme: Base };

const StyledSelectedOwner = styled.div`
  width: fit-content;
  height: 28px;

  display: flex;
  flex-direction: row;
  align-items: center;
  padding: 4px 15px;
  gap: 8px;

  box-sizing: border-box;

  background: ${(props) =>
    props.theme.filterInput.filter.selectedItem.background};

  border-radius: 16px;

  .text {
    color: ${(props) => props.theme.filterInput.filter.selectedItem.color};

    font-weight: 600;
    font-size: 13px;
    line-height: 20px;
  }

  .cross-icon {
    display: flex;
    align-items: center;

    svg {
      cursor: pointer;

      path {
        fill: ${(props) => props.theme.filterInput.filter.selectedItem.color};
      }
    }
  }
`;

StyledSelectedOwner.defaultProps = { theme: Base };

export {
  StyledOwnerInfo,
  StyledPeopleSelectorInfo,
  StyledPeopleSelector,
  StyledAvailableList,
  StyledFooterWrapper,
  StyledSelectedOwner,
  StyledSelectedOwnerContainer,
};
