import styled from "styled-components";
import { hugeMobile } from "../../utils/device";

import { Base } from "../../themes";

const StyledDropzone = styled.div`
  cursor: pointer;
  box-sizing: border-box;
  width: 100%;
  height: 150px;
  border: 2px dashed
    ${(props) => props.theme.createEditRoomDialog.dropzone.borderColor};
  border-radius: 6px;

  position: relative;

  .dropzone_loader {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
  }

  .dropzone {
    height: 100%;
    width: 100%;
    visibility: ${(props) => (props.$isLoading ? "hidden" : "visible")};
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 4px;

    user-select: none;

    &-link {
      display: flex;
      flex-direction: row;
      gap: 4px;

      font-size: 13px;
      line-height: 20px;
      &-main {
        font-weight: 600;
        text-decoration: underline;
        text-decoration-style: dashed;
        text-underline-offset: 1px;
      }
      &-secondary {
        font-weight: 400;
        color: ${(props) =>
          props.theme.createEditRoomDialog.dropzone.linkSecondaryColor};
      }

      @media ${hugeMobile} {
        &-secondary {
          display: none;
        }
      }
    }

    &-exsts {
      font-weight: 600;
      font-size: 12px;
      line-height: 16px;
      color: ${(props) => props.theme.createEditRoomDialog.dropzone.exstsColor};
    }
  }
`;

StyledDropzone.defaultProps = { theme: Base };

export default StyledDropzone;
