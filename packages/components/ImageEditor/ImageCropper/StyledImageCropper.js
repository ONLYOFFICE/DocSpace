import styled from "styled-components";
import { Base } from "../../themes";

const StyledImageCropper = styled.div`
  max-width: 216px;

  .icon_cropper-crop_area {
    width: 216px;
    height: 216px;
    margin-bottom: 4px;
    position: relative;
    .icon_cropper-grid {
      pointer-events: none;
      position: absolute;
      width: 216px;
      height: 216px;
      top: 0;
      bottom: 0;
      left: 0;
      right: 0;
      svg {
        opacity: 0.2;
        path {
          fill: ${(props) =>
            props.theme.createEditRoomDialog.iconCropper.gridColor};
        }
      }
    }
  }

  .icon_cropper-delete_button {
    cursor: pointer;
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    gap: 8px;
    width: 100%;
    padding: 6px 0;
    background: ${(props) =>
      props.theme.createEditRoomDialog.iconCropper.deleteButton.background};
    border: 1px solid
      ${(props) =>
        props.theme.createEditRoomDialog.iconCropper.deleteButton.borderColor};
    border-radius: 3px;
    margin-bottom: 12px;

    transition: all 0.2s ease;
    &:hover {
      background: ${(props) =>
        props.theme.createEditRoomDialog.iconCropper.deleteButton
          .hoverBackground};
      border: 1px solid
        ${(props) =>
          props.theme.createEditRoomDialog.iconCropper.deleteButton
            .hoverBorderColor};
    }

    &-text {
      user-select: none;
      font-weight: 600;
      line-height: 20px;
      color: ${(props) =>
        props.theme.createEditRoomDialog.iconCropper.deleteButton.color};
    }

    svg {
      path {
        fill: ${(props) =>
          props.theme.createEditRoomDialog.iconCropper.deleteButton.iconColor};
      }
    }
  }

  .icon_cropper-zoom-container {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    gap: 12px;
    margin-bottom: 20px;

    &-slider {
      margin: 0;
    }

    &-button {
      user-select: none;
    }
  }
`;

StyledImageCropper.defaultProps = { theme: Base };
export default StyledImageCropper;
