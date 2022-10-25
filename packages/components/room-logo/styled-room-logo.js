import styled, { css } from "styled-components";

import { RoomsType } from "@docspace/common/constants";

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

const editingCss = css`
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

const getRoomTypeCss = (roomType) => {
  switch (roomType) {
    case RoomsType.FillingFormsRoom:
      return formCss;
    case RoomsType.EditingRoom:
      return editingCss;
    case RoomsType.ReviewRoom:
      return reviewCss;
    case RoomsType.ReadOnlyRoom:
      return viewCss;
    case RoomsType.CustomRoom:
      return customCss;
  }
};

const privacyCss = css`
  border: none;

  .room-logo_icon {
    width: 32px;
    height: 32px;
  }
`;

const StyledContainer = styled.div`
  width: 32px;
  height: 32px;

  min-width: 32px;
  min-height: 32px;

  display: flex;
  align-items: center;
  justify-content: center;

  margin-right: 12px;

  .room-logo_checkbox {
    display: none;

    .checkbox {
      margin-right: 0;
    }
  }
`;

const StyledLogoContainer = styled.div`
  width: 32px;
  height: 32px;

  box-sizing: border-box;

  /* border: 2.5px solid black; */
  /* border-radius: 6px; */

  display: flex;
  align-items: center;
  justify-content: center;

  .room-logo_icon {
    width: 32px;
    height: 32px;
  }

  ${(props) => (!props.isPrivacy ? getRoomTypeCss(props.type) : privacyCss)}
  ${(props) => !props.isPrivacy && props.isArchive && archiveCss}
`;

export { StyledContainer, StyledLogoContainer };
