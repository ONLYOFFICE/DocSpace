import styled from "styled-components";

import { hugeMobile } from "@docspace/components/utils/device";

const StyledSMTPContent = styled.div`
  .rectangle-loader_description {
    max-width: 700px;
    margin-bottom: 20px;
    margin-top: 16px;
  }
  .rectangle-loader_title {
    margin-bottom: 8px;
  }
  .rectangle-loader-2 {
    max-width: 350px;
    margin-bottom: 16px;
  }

  .rectangle-loader_checkbox {
    display: flex;
    gap: 8px;
    margin-bottom: 24px;

    svg:first-child {
      margin-top: 2px;
    }
  }
  .rectangle-loader_buttons {
    margin-top: 20px;
    max-width: 404px;
    display: grid;
    grid-template-columns: 86px 1fr 1fr;
    gap: 8px;

    @media ${hugeMobile} {
      grid-template-columns: 1fr;
    }
  }
`;

export { StyledSMTPContent };
