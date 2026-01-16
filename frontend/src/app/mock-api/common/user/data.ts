// /* eslint-disable */
// export const user = {
//     id: 'cfaad35d-07a3-4447-a6c3-d8c3d54fd5df',
//     name: 'Brian Hughes',
//     email: 'hughes.brian@company.com',
//     avatar: 'images/avatars/brian-hughes.jpg',
//     status: 'online',
// };


// Get user data from local storage
let localUser: any = null;
try {
    const stored = localStorage.getItem('user');
    if (stored) {
        localUser = JSON.parse(stored);
    }
} catch (e) {
    // Fallback if parsing fails
    localUser = null;
}

export const user = {
    id: localUser?.id ?? 'cfaad35d-07a3-4447-a6c3-d8c3d54fd5df',
    name: localUser?.displayName || localUser?.user?.name || 'Brian Hughes',

   // email: localUser?.user?.email || 'hughes.brian@company.com',
    // avatar: 'images/avatars/brian-hughes.jpg',
    status: 'online',
};