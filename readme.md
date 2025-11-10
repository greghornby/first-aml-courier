# Comments

- The code assumes units as specified in the tasks. Eg dimensions are specified in cm so i use 10 to represent 10cm, not 0.1 (as if it were meters)

- Tests are a bit copy/paste mess. I intended to have several orders per test but was too much maintenance for a 2 hour task.

- Had a bit of trouble implementing the first test, as Assert.AreEqual doesn't deep compare with lists, but I eventually found the correct method for that.

# Steps for Unfinished Tasks

For step 4, I would implement an "IsHeavy" flag on the input packages to opt in to the new pricing scheme on an individual package basis. This would simply price the individual package at a base of $50, and if over 50kg, just add $1 for every kg over 50.

For step 5, I would add an id to each parcel (this will just be the index it is in the input array), to keep track off parcels as a parcel can't be discounted twice we need a way to track what ones have been discounted. I would create a list of small parcels sort by price (low to high), and the same for medium parcels. Then I would create a sorted list of all parcels sorted by price (low to high).
As every 5th is free, take the total number of packages divided by 5 rounded down, and discount that many starting from the start of the overall sorted list. Track which ones got discounted in a set.
Then iterate through the sorted small and medium lists from the start. Similar thing, divide by 4 rounded down for small, divide by 3 for medium. Iterate that many items from the start of the list, but if a package is already in the set that tracks discounts, then don't count it and keep iterating.
But this assumes an order of which discount gets applied first, in my example, the every 5th package discount applies first. Total savings could vary based on what condition takes precedence for the discount.

# What I Wish I Did Better

Better organization of my struct records. Would've documented them.
Probably a single method that returns all calculated data for a package, size label, cost, overweight cost, since I use very similar switch statements for all.
Maybe some unit test helpers to remove the wall of texts with my expected objects.