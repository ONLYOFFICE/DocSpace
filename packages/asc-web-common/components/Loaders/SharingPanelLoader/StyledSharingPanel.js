import styled from "styled-components";

import { Base } from "@appserver/components/themes";

const StyledContainer = styled.div`
  width: 100%;

  display: flex;
  flex-direction: column;
`;

const StyledHeader = styled.div`
  width: 100%;
  padding: 12px 16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  box-sizing: border-box;

  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};
`;

StyledHeader.defaultProps = { theme: Base };

const StyledExternalLink = styled.div`
  width: 100%;

  display: flex;
  flex-direction: column;

  padding: 20px 16px;

  box-sizing: border-box;

  border-bottom: ${(props) => props.theme.filesPanels.sharing.borderBottom};

  .rectangle-loader {
    margin-bottom: 16px;
  }
`;

StyledExternalLink.defaultProps = { theme: Base };

const StyledInternalLink = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 20px 16px;

  box-sizing: border-box;
`;

const StyledOwner = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  padding: 8px 16px;

  box-sizing: border-box;

  margin-bottom: 16px;

  .owner-info {
    display: flex;
    align-items: center;

    svg:first-child {
      margin-right: 12px;
    }
  }
`;

const StyledBody = styled.div`
  width: 100%;

  display: flex;
  flex-direction: column;

  div:nth-child(3) {
    margin-bottom: 16px;
  }
`;

const StyledItem = styled.div`
  width: 100%;

  display: flex;
  align-items: center;
  justify-content: space-between;

  box-sizing: border-box;

  padding: 8px 16px;

  .item-info {
    display: flex;
    align-items: center;

    svg:first-child {
      margin-right: 12px;
    }
  }
`;

export {
  StyledContainer,
  StyledHeader,
  StyledExternalLink,
  StyledInternalLink,
  StyledOwner,
  StyledBody,
  StyledItem,
};
