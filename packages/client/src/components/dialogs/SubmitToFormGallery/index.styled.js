import { ModalDialog } from "@docspace/components";
import styled from "styled-components";

export const SubmitToGalleryDialog = styled(ModalDialog)`
  .modal-body {
    display: flex;
    flex-direction: column;
    align-items: start;
    justify-content: center;
    gap: 16px;

    font-weight: 400;
    line-height: 20px;
  }
`;

export const FormItem = styled.div`
  width: 100%;
  box-sizing: border-box;
  display: flex;
  flex-direction: row;
  padding: 8px 16px;
  align-items: center;
  justify-content: start;
  gap: 8px;
  border-radius: 6px;
  background: ${(props) => props.theme.infoPanel.history.fileBlockBg};

  .icon {
    margin: 4px;
    width: 24px;
    height: 24px;
    svg {
      width: 24px;
      height: 24px;
    }
  }

  .item-title {
    margin: 8px 0;
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    display: flex;
    min-width: 0;
    gap: 0;

    .name {
      text-overflow: ellipsis;
      white-space: nowrap;
      overflow: hidden;
    }

    .exst {
      flex-shrink: 0;
      color: ${(props) => props.theme.infoPanel.history.fileExstColor};
    }
  }
`;
