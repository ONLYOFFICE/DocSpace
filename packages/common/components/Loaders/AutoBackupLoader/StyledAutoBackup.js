import styled from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledAutoBackup = styled.div`
  width: 100%;
  .auto-backup-loader_main {
    display: grid;
    grid-template-rows: max-content max-content max-content;
    grid-gap: 8px;
    width: 100%;
    .auto-backup-loader_title {
      max-width: 118px;
    }
    .auto-backup-loader_title-description {
      display: block;
      max-width: 700px;
      width: 100%;
      height: 16px;
      @media ${mobile} {
        height: 32px;
      }
    }
    .auto-backup-loader_toggle {
      max-width: 718px;
      height: 64px;
    }
  }
  .auto-backup-loader_menu {
    margin-top: 24px;
    display: grid;
    grid-template-rows: repeat(5, max-content);
    grid-template-columns: 16px 1fr;
    width: 100%;
    grid-column-gap: 8px;
    .auto-backup-loader_option {
      height: 40px;
      max-width: 700px;
      @media ${tablet} {
        height: 54px;
      }
    }
    .auto-backup-loader_option-description {
      margin-top: 8px;
      height: 32px;
      max-width: 350px;
    }
    .auto-backup-loader_option-description-second {
      margin-top: 16px;
      height: 20px;
      max-width: 119px;
    }
    .auto-backup-loader_option-description-third,
    .auto-backup-loader_option-description-fourth {
      margin-top: 4px;
      height: 32px;
      max-width: 350px;
    }
  }
`;

export default StyledAutoBackup;
