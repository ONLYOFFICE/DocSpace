import React from "react";
import styled from "styled-components";
import { Text, Textarea } from "@docspace/components";

const RequestDetailsWrapper = styled.div`
  width: 100%;
`;

export const RequestDetails = () => {
  const requestHeader = `Request URL: https://github.com/Tarantovich/i/settings/hooks/new
Request method: POST 
Accept: */* 
content-type: application/x-www-form-urlencoded
User-Agent: GitHub-Hookshot/b591633 
X-GitHub-Delivery: 79cb2650-9beb-11ed-9425-682162cbe770 
X-GitHub-Event: ping 
X-GitHub-Hook-ID: 398134877 
X-GitHub-Hook-Installation-Target-ID: 592662620 
X-GitHub-Hook-Installation-Target-Type: repository`;
  const requestBody =
    '{"menu": {"id": "file","value": "File","popup": {"menuitem": [{"value": "New", "onclick": "CreateNewDoc()"},{"value": "Open", "onclick": "OpenDoc()"},{"value": "Close", "onclick": "CloseDoc()"}]}}}';

  return (
    <RequestDetailsWrapper>
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px" }}>
        Request post header
      </Text>
      <Textarea value={requestHeader} showCopyIcon hasNumeration isFullHeight isLimitedWidth />
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px", marginTop: "16px" }}>
        Request post body
      </Text>
      <Textarea
        value={requestBody}
        isJSONField
        showCopyIcon
        hasNumeration
        isFullHeight
        isLimitedWidth
      />
    </RequestDetailsWrapper>
  );
};
