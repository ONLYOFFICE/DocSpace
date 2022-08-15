import styled, { css } from "styled-components";

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
      display: flex;
      flex-direction: row;
      align-items: center;
      gap: 6px;

      .set_room_params-info-title-text {
        font-weight: 600;
        font-size: 13px;
        line-height: 20px;
      }
    }
    .set_room_params-info-description {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: #a3a9ae;
    }
  }

  .set_room_params-toggle {
    width: 28px;
    height: 16px;
    margin: 2px 0;
  }
`;

export { StyledParam };
