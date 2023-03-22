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

  @media ${mobile} {
    margin-top: 32px;
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

export const StyledHeader = styled.div`
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
    padding-bottom: 40px;
  }

  @media ${hugeMobile} {
    margin-top: 0;
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
    padding-bottom: 40px;
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

export const ButtonsWrapper = styled.div`
  display: flex;
  flex-direction: row;
  gap: 16px;
  width: 100%;
`;
