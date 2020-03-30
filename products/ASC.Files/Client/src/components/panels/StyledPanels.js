import styled from "styled-components";

const StyledPanel = styled.div`
  .header_aside-panel {
    transform: translateX(${props => (props.visible ? "0" : "500px")});
    width: 500px;

    @media (max-width: 550px) {
      width: 320px;
      transform: translateX(${props => (props.visible ? "0" : "320px")});
    }
  }
`;

const StyledContent = styled.div`
  box-sizing: border-box;
  position: relative;
  width: 100%;
  background-color: #fff;
  padding: 0 16px 16px;

  .header_aside-panel-header {
    max-width: 500px;
    margin: 0 0 0 16px;
    line-height: 56px;
    font-weight: 700;
  }

  .header_aside-panel-plus-icon {
    margin-left: auto;
  }
`;

const StyledHeaderContent = styled.div`
  display: flex;
  align-items: center;
`;

const StyledBody = styled.div`
  .selector-wrapper {
    position: fixed;
    height: 94%;

    .column-options {
      padding: 0 0 16px 0;
      width: 470px;

      @media (max-width: 550px) {
        width: 320px;
        padding: 0 28px 16px 0;
      }

      .body-options {
        padding-top: 16px;
      }
    }
    .footer {
      @media (max-width: 550px) {
        padding: 16px 28px 16px 0;
      }
    }
  }
`;

const StyledSharingHeaderContent = styled.div`
  display: flex;
  align-items: center;
  border-bottom: 1px solid #dee2e6;

  .sharing_panel-icons-container {
    display: flex;
    margin-left: auto;

    .sharing_panel-drop-down-wrapper {
      position: relative;

      .sharing_panel-drop-down {
        padding: 8px 16px;
      }
      .sharing_panel-plus-icon {
        margin-right: 12px;
      }
    }
  }
`;

const StyledSharingBody = styled.div`
  position: relative;
  padding: 16px 0;

  .sharing_panel-remove-icon {
    margin-left: auto;
    width: 18px;
    height: 18px;
  }
`;

const StyledFooter = styled.div`
  display: flex;
  position: fixed;
  bottom: 0;
  padding: 16px 0;
  width: 94%;
  background-color: #fff;

  .sharing_panel-button {
    margin-left: auto;
  }

  @media (max-width: 550px) {
    width: 90%;
  }
`;

export {
  StyledPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledSharingHeaderContent,
  StyledSharingBody,
  StyledFooter
};
