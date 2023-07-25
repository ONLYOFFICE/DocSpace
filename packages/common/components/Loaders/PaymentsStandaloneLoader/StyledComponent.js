import styled from "styled-components";

import { mobile, tablet } from "@docspace/components/utils/device";
const StyledEnterpriseComponent = styled.div`
  .payments-loader_4,
  .payments-loader_2,
  .payments-loader_3,
  .payments-loader_5,
  .payments-loader_6,
  .payments-loader_7,
  .payments-loader_8,
  .payments-loader_9,
  .payments-loader_10 {
    display: block;
  }

  .payments-loader_4,
  .payments-loader_9 {
    height: 32px;
    @media ${tablet} {
      height: 40px;
    }
  }

  .payments-loader_1 {
    max-width: 272px;
    margin-bottom: 12px;
  }
  .payments-loader_2 {
    max-width: 660px;
    margin-bottom: 12px;
    height: 32px;

    @media ${mobile} {
      height: 64px;
    }
  }
  .payments-loader_3 {
    max-width: 342px;
    margin-bottom: 16px;

    @media ${mobile} {
      max-width: 100%;
    }
  }
  .payments-loader_4 {
    max-width: 110px;
    margin-bottom: 16px;
  }
  .payments-loader_5 {
    max-width: 497px;
    margin-bottom: 32px;

    height: 32px;

    @media ${mobile} {
      height: 60px;
    }
  }
  .payments-loader_6 {
    max-width: 126px;
    margin-bottom: 12px;
  }
  .payments-loader_7 {
    max-width: 581px;
    margin-bottom: 16px;
    height: 16px;
    @media ${mobile} {
      height: 32px;
    }
  }
  .payments-loader_8 {
    max-width: 350px;
    margin-bottom: 16px;
  }
  .payments-loader_9 {
    max-width: 110px;
    margin-bottom: 20px;

    @media ${mobile} {
      max-width: 100%;
    }
  }
  .payments-loader_10 {
    max-width: 264px;
    margin-bottom: 4px;
  }
  .payments-loader_11 {
    max-width: 304px;
  }
`;

const StyledTrialComponent = styled.div`
  .payments-loader_4,
  .payments-loader_2,
  .payments-loader_3,
  .payments-loader_5,
  .payments-loader_6,
  .payments-loader_7,
  .payments-loader_8,
  .payments-loader_9 {
    display: block;
  }

  .payments-loader_1 {
    max-width: 660px;
    margin-bottom: 20px;
    height: 294px;

    @media ${mobile} {
      height: 380px;
    }
  }
  .payments-loader_2 {
    max-width: 497px;
    margin-bottom: 12px;
  }
  .payments-loader_3 {
    max-width: 206px;
    margin-bottom: 4px;
  }
  .payments-loader_4 {
    max-width: 172px;
    margin-bottom: 4px;
  }
  .payments-loader_5 {
    max-width: 219px;
    margin-bottom: 12px;
  }
  .payments-loader_6 {
    max-width: 338px;
    margin-bottom: 4px;
  }
  .payments-loader_7 {
    max-width: 197px;
    margin-bottom: 16px;
  }
  .payments-loader_8 {
    max-width: 350px;
    margin-bottom: 16px;
  }
  .payments-loader_9 {
    max-width: 255px;
    margin-bottom: 4px;

    @media ${mobile} {
      max-width: 100%;
    }
  }
`;
export { StyledEnterpriseComponent, StyledTrialComponent };
