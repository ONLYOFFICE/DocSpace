import styled from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledDataBackup = styled.div`
  width: 100%;
  .data-backup-loader_main {
    display: grid;
    grid-template-rows: 1fr;
    grid-gap: 8px;
    width: 100%;
    .data-backup-loader_title {
      max-width: 118px;
    }
    .data-backup-loader_title-description {
      display: block;
      max-width: 700px;
      width: 100%;
      height: 16px;
      @media ${mobile} {
        height: 32px;
      }
    }
  }
  .data-backup-loader {
    margin-top: 24px;
    display: grid;
    grid-template-rows: repeat(5, max-content);
    grid-template-columns: 16px 1fr;
    width: 100%;
    grid-column-gap: 8px;
    .data-backup-loader_menu,
    .data-backup-loader_menu-higher,
    .data-backup-loader_menu-last {
      height: 40px;
      max-width: 700px;
      width: 100%;
      margin-bottom: 16px;
    }
    .data-backup-loader_menu-higher {
      height: 72px;
      @media ${mobile} {
        height: 120px;
      }
    }
    .data-backup-loader_menu-last {
      height: 56px;
      @media ${mobile} {
        height: 88px;
      }
    }
    .data-backup-loader_menu-description {
      margin-bottom: 16px;
      height: 32px;
      max-width: 285px;
      width: 100%;
      @media ${tablet} {
        height: 40px;
      }
    }
  }
`;

export default StyledDataBackup;
