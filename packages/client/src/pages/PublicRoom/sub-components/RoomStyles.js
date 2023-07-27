import styled from "styled-components";
import { hugeMobile, mobile, tablet } from "@docspace/components/utils/device";

export const StyledPage = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 0 auto;
  max-width: 960px;
  box-sizing: border-box;

  @media ${tablet} {
    padding: 0 16px;
  }

  @media ${hugeMobile} {
    margin: 32px 0;
    padding: 0 8px 0 20px;
  }

  @media ${mobile} {
    padding: 0 8px 0 16px;
  }

  .subtitle {
    margin-bottom: 32px;
  }

  .password-form {
    width: 100%;
    margin-bottom: 8px;
  }

  .subtitle {
    margin-bottom: 32px;
  }

  .public-room-content {
    padding-top: 9%;
    justify-content: unset;
    min-height: unset;

    .public-room-text {
      margin: 8px 0;
    }

    .public-room-name {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 32px;
    }

    .public-room-icon {
      min-width: 32px;
      min-height: 32px;
    }

    .public-room-text {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
`;

export const StyledContent = styled.div`
  min-height: 100vh;
  flex: 1 0 auto;
  flex-direction: column;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  -webkit-box-orient: vertical;
  -webkit-box-direction: normal;

  @media ${hugeMobile} {
    justify-content: start;
    min-height: 100%;
  }
`;

export const StyledBody = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 56px auto;

  @media ${hugeMobile} {
    width: 100%;
    margin: 0 auto;
  }

  .title {
    margin-bottom: 32px;
    text-align: center;
  }

  .subtitle {
    margin-bottom: 32px;
  }

  .docspace-logo {
    display: flex;
    align-items: center;
    justify-content: center;
    padding-bottom: 64px;
  }

  .password-field-wrapper {
    width: 100%;
  }

  .password-change-form {
    margin-top: 32px;
    margin-bottom: 16px;
  }

  .phone-input {
    margin-bottom: 24px;
  }

  .delete-profile-confirm {
    margin-bottom: 8px;
  }

  .phone-title {
    margin-bottom: 8px;
  }
`;
