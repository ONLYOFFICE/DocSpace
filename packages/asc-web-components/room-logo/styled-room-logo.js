import styled, { css } from "styled-components";

const formCss = css`
  border-color: #26acb8;

  svg {
    path {
      fill: #26acb8;
    }
  }
`;

const archiveCss = css`
  border-color: #a3a9ae;

  svg {
    path {
      fill: #a3a9ae;
    }
  }
`;

const collaborationCss = css`
  border-color: #eb7b0c;

  svg {
    path {
      fill: #eb7b0c;
    }
  }
`;

const reviewCss = css`
  border-color: #cc5bcc;

  svg {
    path {
      fill: #cc5bcc;
    }
  }
`;

const viewCss = css`
  border-color: #2869a9;

  svg {
    path {
      fill: #2869a9;
    }
  }
`;

const customCss = css`
  border-color: #f2557c;

  svg {
    path {
      fill: #f2557c;
    }
  }
`;

const privacyCss = css`
  border: none;

  .room-logo_icon {
    width: 32px;
    height: 32px;
  }
`;

const StyledLogoContainer = styled.div`
  width: 32px;
  height: 32px;

  box-sizing: border-box;

  border: 2px solid black;
  border-radius: 6px;

  display: flex;
  align-items: center;
  justify-content: center;

  .room-logo_icon {
    width: 16px;
    height: 16px;
  }

  ${(props) => !props.isPrivacy && props.type === "view" && viewCss}
  ${(props) => !props.isPrivacy && props.type === "fill" && formCss}
  ${(props) => !props.isPrivacy && props.type === "editing" && collaborationCss}
  ${(props) => !props.isPrivacy && props.type === "review" && reviewCss}
  ${(props) => !props.isPrivacy && props.type === "custom" && customCss}
  ${(props) => !props.isPrivacy && props.type === "archive" && archiveCss}

  ${(props) => props.isPrivacy && privacyCss}
`;

export default StyledLogoContainer;
