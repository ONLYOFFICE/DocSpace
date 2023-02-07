export function getGroupList() {
  return Promise.resolve([
    {
      id: "group-administration",
      name: "Administration",
    },
    {
      id: "group-dev",
      name: "Development",
    },
    {
      id: "group-management",
      name: "Management",
    },
    {
      id: "group-marketing",
      name: "Marketing",
    },
    {
      id: "group-mobile",
      name: "Mobile",
    },
    {
      id: "group-support",
      name: "Support",
    },
    {
      id: "group-web",
      name: "Web",
    },
  ]);
}
