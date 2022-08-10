import styled from "styled-components";
import { mobile, tablet } from "@docspace/components/utils/device";

export const StyledPage = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  margin: 56px auto 0 auto;
  max-width: 960px;

  @media ${tablet} {
    padding: 0 16px;
  }

  @media ${mobile} {
    margin-top: 72px;
  }
`;

export const StyledHeader = styled.div`
  text-align: left;

  .title {
    margin-bottom: 24px;
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
`;

export const StyledBody = styled.div`
  width: 320px;

  @media ${tablet} {
    justify-content: center;
  }

  @media ${mobile} {
    width: 100%;
  }

  .form-field {
    height: 48px;
  }

  .password-field-wrapper {
    width: 100%;
  }

  .confirm-button {
    width: 100%;
    margin-top: 8px;
  }

  .password-change-form {
    margin-top: 32px;
    margin-bottom: 16px;
  }

  .confirm-subtitle {
    margin-bottom: 8px;
  }

  .info-delete {
    margin-bottom: 24px;
  }

  .phone-input {
    margin-top: 32px;
    margin-bottom: 16px;
  }
`;

export const ButtonsWrapper = styled.div`
  display: flex;
  flex: 1fr 1fr;
  flex-direction: row;
  gap: 16px;

  .button {
    width: 100%;
  }
`;
