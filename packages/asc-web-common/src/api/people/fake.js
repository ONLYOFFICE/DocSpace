import Filter from "./filter";
import uniqueId from "lodash/uniqueId";

const generateUsers = (count, search, group) => {
  return Array.from({ length: count }, () => {
    const user = {
      id: uniqueId(),
      groups: group ? [group] : [],
      displayName: "Demo User",
      avatar: "",
      title: "Demo",
      email: "demo@demo.com",
    };

    return user;
  });
};

/* key: u.id,
groups: u.groups || [],
label: u.displayName,
email: u.email,
position: u.title,
avatarUrl: u.avatar
*/

export function getUserList(filter = Filter.getDefault()) {
  const fakeUsers = generateUsers(
    filter.pageCount,
    filter.search,
    filter.group
  );
  return Promise.resolve({ items: fakeUsers, total: 1000 });
}
