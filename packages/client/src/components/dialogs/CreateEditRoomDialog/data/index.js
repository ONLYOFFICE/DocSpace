import { RoomsType } from "@docspace/common/constants";

export const roomTypes = [
  {
    id: "shared_filling-forms-room",
    type: RoomsType.FillingFormsRoom,
    title: "FillingFormsRoomTitle",
    description: "FillingFormsRoomDescription",
    defaultTag: "Files:FillingFormRooms",
    withSecondaryInfo: true,
    secondaryInfo: "FillingFormsRoomSecondaryInfo",
  },
  {
    id: "shared_collaboration-room",
    type: RoomsType.EditingRoom,
    title: "CollaborationRoomTitle",
    description: "CollaborationRoomDescription",
    defaultTag: "Files:CollaborationRooms",
    withSecondaryInfo: true,
    secondaryInfo: "CollaborationRoomSecondaryInfo",
  },
  {
    id: "shared_review-room",
    type: RoomsType.ReviewRoom,
    title: "ReviewRoomTitle",
    description: "ReviewRoomDescription",
    defaultTag: "Common:Review",
    withSecondaryInfo: true,
    secondaryInfo: "ReviewRoomSecondaryInfo",
  },
  {
    id: "shared_read-only-room",
    type: RoomsType.ReadOnlyRoom,
    title: "ViewOnlyRoomTitle",
    description: "ViewOnlyRoomDescription",
    defaultTag: "Files:ViewOnlyRooms",
    withSecondaryInfo: true,
    secondaryInfo: "ViewOnlyRoomSecondaryInfo",
  },
  {
    id: "shared_custom-room",
    type: RoomsType.CustomRoom,
    title: "CustomRoomTitle",
    description: "CustomRoomDescription",
    defaultTag: "Files:CustomRooms",
    withSecondaryInfo: false,
  },
];
