import styled from "styled-components";
import { smallTablet, mobile } from "@docspace/components/utils/device";

export const DeleteDataLayout = styled.div`
  width: 100%;

  hr {
    margin: 24px 0;
    border: none;
    border-top: 1px solid #eceef1;
  }
`;

export const MainContainer = styled.div`
  max-width: 700px;
  white-space: pre-line;

  .header {
    margin-bottom: 8px;
  }

  .description {
    margin-bottom: 16px;
  }

  .helper {
    margin-bottom: 24px;
    color: ${(props) => props.theme.client.settings.common.descriptionColor};
  }
`;

export const ButtonWrapper = styled.div`
  display: flex;
  gap: 8px;
  align-items: center;

  .request-again-link {
    margin-left: 4px;
  }

  @media (${smallTablet}) {
    flex-direction: column-reverse;
    gap: 16px;
    position: absolute;
    bottom: 16px;
    width: calc(100% - 40px);

    @media (${mobile}) {
      width: calc(100% - 32px);
    }

    .button {
      width: 100%;
    }
  }
`;
