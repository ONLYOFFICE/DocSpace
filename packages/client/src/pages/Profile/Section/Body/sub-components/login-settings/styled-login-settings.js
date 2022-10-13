import styled from "styled-components";
import { hugeMobile } from "@docspace/components/utils/device";

export const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 16px;

  .header {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .actions {
    display: flex;
    gap: 16px;
    align-items: center;

    @media ${hugeMobile} {
      flex-direction: column;
      gap: 12px;
      align-items: flex-start;

      .button {
        width: 100%;
        height: 40px;
        font-size: 14px;
      }
    }
  }
`;
