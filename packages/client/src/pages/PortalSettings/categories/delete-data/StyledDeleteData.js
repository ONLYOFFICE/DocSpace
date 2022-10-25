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
    color: #657077;
  }

  .button {
    @media (${smallTablet}) {
      position: absolute;
      bottom: 16px;
      width: calc(100% - 40px);

      @media (${mobile}) {
        width: calc(100% - 32px);
      }
    }
  }
`;
