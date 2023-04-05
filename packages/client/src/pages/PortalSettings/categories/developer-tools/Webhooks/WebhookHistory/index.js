import React from "react";
import styled from "styled-components";

import HistoryHeader from "./sub-components/HistoryHeader";
import HistoryFilterHeader from "./sub-components/HistoryFilterHeader";
import WebhookHistoryTable from "./sub-components/WebhookHistoryTable";

const WebhookWrapper = styled.div`
  width: 100%;
`;

const WebhookHistory = () => {
  const historyWebhooks = [
    {
      id: 279,
      configName: "localhook",
      creationTime: "2023-04-01T10:56:14",
      requestHeaders:
        '{"Accept": ["*/*"], "x-docspace-signature-256": ["sha256=F41535FCEDEF397A51C750430949C75111907B83B7487EDCE3C8D3DFEF8F1AB0"]}',
      requestPayload:
        '{"response":{"parentId":1,"filesCount":0,"foldersCount":0,"new":0,"mute":false,"tags":[],"logo":{"original":"","large":"","medium":"","small":""},"pinned":false,"roomType":2,"private":false,"id":14,"rootFolderId":1,"canShare":true,"security":{"Read":true,"Create":true,"Delete":false,"EditRoom":true,"Rename":true,"CopyTo":true,"Copy":false,"MoveTo":true,"Move":true,"Pin":true,"Mute":true,"EditAccess":true,"Duplicate":true},"title":"aaa","access":0,"shared":false,"created":"2023-04-01T15:56:14.0000000+05:00","createdBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false},"updated":"2023-04-01T15:56:14.0000000+05:00","rootFolderType":14,"updatedBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false}},"count":1,"links":[{"href":"http://localhost:8092/api/2.0/files/rooms","action":"POST"}],"status":0,"statusCode":200}',
      responseHeaders:
        '{"Date": ["Sat, 01 Apr 2023 10:56:18 GMT"], "ETag": ["W/\\"20-YDeHRIkWJhpJXxkUn/H0kuUGe1c\\""], "Connection": ["keep-alive"], "Keep-Alive": ["timeout=5"], "X-Powered-By": ["Express"]}',
      responsePayload: '{"message":"okay, done posting"}',
      status: 200,
      delivery: "2023-04-01T10:56:18",
    },
    {
      id: 278,
      configName: "test123",
      creationTime: "2023-04-01T10:56:14",
      requestHeaders:
        '{"Accept": ["*/*"], "x-docspace-signature-256": ["sha256=F41535FCEDEF397A51C750430949C75111907B83B7487EDCE3C8D3DFEF8F1AB0"]}',
      requestPayload:
        '{"response":{"parentId":1,"filesCount":0,"foldersCount":0,"new":0,"mute":false,"tags":[],"logo":{"original":"","large":"","medium":"","small":""},"pinned":false,"roomType":2,"private":false,"id":14,"rootFolderId":1,"canShare":true,"security":{"Read":true,"Create":true,"Delete":false,"EditRoom":true,"Rename":true,"CopyTo":true,"Copy":false,"MoveTo":true,"Move":true,"Pin":true,"Mute":true,"EditAccess":true,"Duplicate":true},"title":"aaa","access":0,"shared":false,"created":"2023-04-01T15:56:14.0000000+05:00","createdBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false},"updated":"2023-04-01T15:56:14.0000000+05:00","rootFolderType":14,"updatedBy":{"id":"66faa6e4-f133-11ea-b126-00ffeec8b4ef","displayName":" Administrator","avatarSmall":"/static/images/default_user_photo_size_32-32.png?hash=1587389534","profileUrl":"http://localhost:8092/accounts/view/administrator","hasAvatar":false}},"count":1,"links":[{"href":"http://localhost:8092/api/2.0/files/rooms","action":"POST"}],"status":0,"statusCode":200}',
      responseHeaders:
        '{"Date": ["Sat, 01 Apr 2023 10:56:18 GMT"], "Server": ["nginx/1.21.1"], "Connection": ["keep-alive"]}',
      responsePayload: "",
      status: 405,
      delivery: "2023-04-01T10:56:18",
    },
  ];

  return (
    <WebhookWrapper>
      <HistoryHeader />
      <main>
        <HistoryFilterHeader />
        <WebhookHistoryTable historyWebhooks={historyWebhooks} />
      </main>
    </WebhookWrapper>
  );
};

export default WebhookHistory;
