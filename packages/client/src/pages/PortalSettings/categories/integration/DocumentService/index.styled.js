import { mobile } from "@docspace/components/utils/device";
import styled from "styled-components";

export const Location = styled.div`
  display: flex;
  flex-direction: column;
  gap: 20px;
`;

export const LocationHeader = styled.div`
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin: 5px 0 2px 0;

  .main {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 4px;

    .heading {
      margin: 0;
      font-size: 16px;
      font-weight: 700;
      line-height: 22px;
    }

    .help-button-wrapper {
      width: 12px;
      height: 12px;
    }
  }

  .secondary {
    width: 100%;
    max-width: 700px;
    font-size: 13px;
    font-weight: 400;
    line-height: 20px;
  }
`;

export const LocationForm = styled.form`
  display: flex;
  flex-direction: column;
  gap: 20px;

  .form-inputs {
    width: 100%;
    max-width: 350px;

    display: flex;
    flex-direction: column;
    gap: 16px;

    .input-wrapper {
      display: flex;
      flex-direction: column;
      gap: 4px;

      .icon-button {
        display: none;
      }
    }

    .subtitle {
      color: ${(props) => props.theme.client.settings.common.descriptionColor};
    }
  }

  .form-buttons {
    display: flex;
    flex-direction: row;
    gap: 8px;

    @media ${mobile} {
      .button {
        width: 100%;
      }
    }
  }
`;
