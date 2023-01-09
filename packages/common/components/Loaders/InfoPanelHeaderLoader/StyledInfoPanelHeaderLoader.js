import styled from "styled-components";
import { Base } from "@docspace/components/themes";
import { tablet } from "@docspace/components/utils/device";

const getHeaderHeight = ({ withSubmenu, isTablet }) => {
  let res = isTablet ? 53 : 69;
  if (withSubmenu) res += 32;
  return `${res}px`;
};

const getMainHeight = ({ withSubmenu, isTablet }) => {
  let res = isTablet ? 52 : 68;
  if (withSubmenu) res += 1;
  return `${res}px`;
};

const StyledInfoPanelHeader = styled.div`
  width: 100%;
  max-width: 100%;

  height: ${(props) => getHeaderHeight(props)};
  min-height: ${(props) => getHeaderHeight(props)};
  @media ${tablet} {
    height: ${(props) => getHeaderHeight({ ...props, isTablet: true })};
    min-height: ${(props) => getHeaderHeight({ ...props, isTablet: true })};
  }

  display: flex;
  flex-direction: column;
  border-bottom: ${(props) =>
    props.withSubmenu
      ? "none"
      : `1px solid ${props.theme.infoPanel.borderColor}`};
  .main {
    padding: 0 20px;
    box-sizing: border-box;
    height: ${(props) => getMainHeight(props)};
    min-height: ${(props) => getMainHeight(props)};
    @media ${tablet} {
      height: ${(props) => getMainHeight({ ...props, isTablet: true })};
      min-height: ${(props) => getMainHeight({ ...props, isTablet: true })};
    }

    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
  }
  .submenu {
    display: flex;
    height: 32px;
    width: 100%;
    justify-content: center;
    align-items: center;
    gap: 40px;
    border-bottom: ${(props) =>
      `1px solid ${props.theme.infoPanel.borderColor}`};
  }
`;

StyledInfoPanelHeader.defaultProps = { theme: Base };

export { StyledInfoPanelHeader };
