import styled from "styled-components";
import Base from "../themes/base";

const StyledAccessRightWrapper = styled.div`
  display: flex;
  align-items: center;

  .access-right__icon {
    display: flex;
    align-items: center;
    path {
      fill: ${(props) => props.theme.dropDownItem.icon.color};
    }
  }

  .combo-button {
    padding-left: 4px;
  }
`;
StyledAccessRightWrapper.defaultProps = { theme: Base };

const StyledAccessRightIcon = styled.img`
  margin-right: 4px;
`;

const StyledAccessRightItem = styled.div`
  width: auto;

  display: flex;
  align-items: flex-start;
  justify-content: flex-start;
  align-content: center;

  padding: 7px 0px;

  line-height: 16px;
  font-style: normal;
`;

const StyledAccessRightDescriptionItem = styled.div`
  margin: 1px 0px;

  font-size: 13px;
  font-style: normal;
  font-weight: 400;
  line-height: 16px;
  color: #a3a9ae;
`;

const StyledAccessRightItemIcon = styled.img`
  margin-right: 8px;
`;

const StyledAccessRightItemContent = styled.div`
  width: 100%;
  white-space: normal;
`;

const StyledAccessRightItemTitleAndBadge = styled.div`
  display: flex;
  align-items: center;
`;

StyledAccessRightItem.defaultProps = { theme: Base };
export {
  StyledAccessRightItem,
  StyledAccessRightDescriptionItem,
  StyledAccessRightItemIcon,
  StyledAccessRightItemContent,
  StyledAccessRightItemTitleAndBadge,
  StyledAccessRightWrapper,
  StyledAccessRightIcon,
};
