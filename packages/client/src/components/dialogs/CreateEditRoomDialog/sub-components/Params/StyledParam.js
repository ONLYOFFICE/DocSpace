import styled, { css } from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledParam = styled.div`
  box-sizing: border-box;
  display: flex;
  width: 100%;

  ${(props) =>
    props.storageLocation
      ? css``
      : props.folderName
      ? css`
          flex-direction: column;
          gap: 4px;
        `
      : ""}

  .set_room_params-info {
    display: flex;
    flex-direction: column;
    gap: 4px;

    .set_room_params-info-title {
      user-select: none;
      display: flex;
      flex-direction: row;
      align-items: center;
      gap: 6px;

      .set_room_params-info-title-text {
        user-select: none;
        font-weight: 600;
        font-size: 13px;
        line-height: 20px;
      }
    }
    .set_room_params-info-description {
      user-select: none;
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: ${(props) =>
        props.theme.createEditRoomDialog.commonParam.descriptionColor};
    }
  }

  .set_room_params-toggle {
    width: 28px;
    height: 16px;
    margin: 2px 0;
  }
`;

StyledParam.defaultProps = { theme: Base };

export { StyledParam };
