import React from "react";
import styled from "styled-components";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

const StyledContainer = styled.div`
  width: 100%;
`;

const StyledSeparator = styled.div`
  width: 100%;
  height: 1px;

  margin: 24px 0;

  background-color: #eceef1;
`;

const StyledInfo = styled.div`
  margin-top: 24px;

  width: 100%;
  height: auto;

  display: grid;

  grid-template-columns: max-content 1fr;

  gap: 8px 24px;
`;

const Info = ({ t, plugin }) => {
  console.log(plugin);
  return (
    <StyledContainer>
      <StyledSeparator />
      <Text fontSize={"14px"} fontWeight={600} lineHeight={"16px"} noSelect>
        {t("Metadata")}
      </Text>
      <StyledInfo>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Author
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {plugin?.author}
        </Text>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Version
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {plugin?.version}
        </Text>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Uploader
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {plugin?.createBy}
        </Text>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Upload date
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {plugin?.createOn}
        </Text>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Status
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          Not need enter settings
        </Text>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Homepage
        </Text>
        <Link
          fontSize={"13px"}
          fontWeight={600}
          lineHeight={"20px"}
          type={"page"}
          href={plugin?.homePage}
          target={"_blank"}
          noSelect
          isHovered
        >
          {plugin?.homePage}
        </Link>
        <Text
          fontSize={"13px"}
          fontWeight={400}
          lineHeight={"20px"}
          noSelect
          truncate
        >
          Description
        </Text>
        <Text fontSize={"13px"} fontWeight={600} lineHeight={"20px"} noSelect>
          {plugin?.description}
        </Text>
      </StyledInfo>
    </StyledContainer>
  );
};

export default Info;
