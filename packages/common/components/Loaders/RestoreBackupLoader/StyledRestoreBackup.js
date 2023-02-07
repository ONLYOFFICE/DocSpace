import styled from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

const StyledRestoreBackup = styled.div`
  width: 100%;
  .restore-backup-loader_title {
    max-width: 400px;
    height: 12px;
    @media ${mobile} {
      height: 30px;
    }
  }
  .restore-backup_checkbox {
    margin-top: 24px;
    margin-bottom: 24px;
    display: grid;
    grid-template-rows: repeat(3, max-content);
    grid-template-columns: 16px 1fr;
    grid-column-gap: 8px;
    grid-row-gap: 16px;
    .restore-backup_checkbox-first {
      max-width: 61px;
      height: 20px;
    }
    .restore-backup_checkbox-second {
      max-width: 418px;
      height: 20px;
      @media ${mobile} {
        height: 40px;
      }
    }
    .restore-backup_checkbox-third {
      max-width: 122px;
      height: 20px;
    }
  }
  .restore-backup_input {
    max-width: 350px;
    margin-bottom: 16px;
  }
  .restore-backup_backup-list {
    max-width: 130px;
    display: block;
  }
  .restore-backup_notification {
    margin-bottom: 24px;
    margin-top: 11px;
    display: grid;
    // grid-template-rows: repeat(3, max-content);
    grid-template-columns: 16px 1fr;
    grid-column-gap: 8px;
    grid-row-gap: 16px;
    .restore-backup_notification-title {
      max-width: 315px;
    }
  }
  .restore-backup_warning-title {
    max-width: 72px;
  }
  .restore-backup_warning-description {
    display: block;
    height: 32px;
    max-width: 700px;
    @media ${mobile} {
      height: 48px;
    }
  }
  .restore-backup_warning {
    margin-top: 17px;
    margin-bottom: 24px;
    display: block;
    height: 20px;
    max-width: 700px;
    @media ${mobile} {
      height: 31px;
    }
  }
  .restore-backup_button {
    display: block;
    max-width: 100px;
    height: 32px;
    @media ${mobile} {
      height: 40px;
      max-width: 100%;
    }
  }
`;

export default StyledRestoreBackup;
