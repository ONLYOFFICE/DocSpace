import React from "react";
import styled from "styled-components";
import { Text, Textarea } from "@docspace/components";

const RequestDetailsWrapper = styled.div`
  width: 100%;
`;

export const RequestDetails = () => {
  const requestHeader =
    '{"Accept": ["*/*"], "x-docspace-signature-256": ["sha256=AE7E0F511A76DD6DE81A5CB194721E2B99120115AC8F3F9BE06DB19609EC854F"]}';
  const requestBody =
    '{"response":{"folderId":2,"version":1,"versionGroup":1,"contentLength":"1,08 Мб","pureContentLength":1132381,"fileStatus":0,"mute":false,"viewUrl":"http://localhost:8092/filehandler.ashx?action=download&fileid=5","webUrl":"http://localhost:8092/doceditor?fileid=5&version=1","fileType":6,"fileExst":".pptx","comment":"Создано","thumbnailStatus":0,"denyDownload":false,"denySharing":false,"viewAccessability":{"ImageView":false,"MediaView":false,"WebView":true,"WebEdit":true,"WebReview":false,"WebCustomFilterEditing":false,"WebRestrictedEditing":false,"WebComment":true,"CoAuhtoring":true,"Convert":false},"id":5,"rootFolderId":2,"canShare":true,"security":{"Read":true,"Comment":true,"FillForms":true,"Review":true,"Edit":true,"Delete":true,"CustomFilter":true,"Rename":true,"ReadHistory":true,"Lock":false,"EditHistory":true,"Copy":true,"Move":true,"Duplicate":true},"title":"ONLYOFFICE Sample Presentation.pptx","access":0,"shared":false,"created":"2023-03-21T12:32:13.0000000+05:00","createdBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false},"updated":"2023-03-21T12:32:13.0000000+05:00","rootFolderType":5,"updatedBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false}},"count":1,"links":[{"href":"http://localhost:8092/api/2.0/files/file/5/recent","action":"POST"}],"status":0,"statusCode":200}';

  return (
    <RequestDetailsWrapper>
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px" }}>
        Request post header
      </Text>
      <Textarea value={requestHeader} enableCopy hasNumeration isFullHeight isJSONField />
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px", marginTop: "16px" }}>
        Request post body
      </Text>
      <Textarea value={requestBody} isJSONField enableCopy hasNumeration isFullHeight />
    </RequestDetailsWrapper>
  );
};
