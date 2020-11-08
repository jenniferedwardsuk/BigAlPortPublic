using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUIController : MonoBehaviour
{
	[SerializeField] Text factText;
	[SerializeField] Image factImage;
	[SerializeField] Dropdown speciesDropdown;
	[SerializeField] Button closeButton;
	[SerializeField] List<FactfileImage> factfileImages = new List<FactfileImage>()
	{
		new FactfileImage(){ imageName = "dragonfly.jpg" },
		new FactfileImage(){ imageName = "centipede.jpg" },
		new FactfileImage(){ imageName = "scorpion.jpg" },
		new FactfileImage(){ imageName = "frog.jpg" },
		new FactfileImage(){ imageName = "lizard.jpg" },
		new FactfileImage(){ imageName = "tuatara.jpg" },
		new FactfileImage(){ imageName = "crocodile.jpg" },
		new FactfileImage(){ imageName = "pterosaur.gif" },
		new FactfileImage(){ imageName = "dryosaurus.gif" },
		new FactfileImage(){ imageName = "othnielia.gif" },
		new FactfileImage(){ imageName = "ornitholestes.gif" },
		new FactfileImage(){ imageName = "stegosaurus.gif" },
		new FactfileImage(){ imageName = "diplodocus.jpg" },
		new FactfileImage(){ imageName = "allosaurus.gif" },
	};
	bool initted = false;

	[System.Serializable]
	private class FactfileImage
	{
		[SerializeField] public string imageName;
		[SerializeField] public Sprite sprite;
	}

	private void Awake()
	{
		if (!initted)
		{
			Init();
		}
	}

	private void Init()
	{ 
		var dropdownOptions = new List<Dropdown.OptionData>();
		foreach (var item in BigAl_pl.species)
		{
			if (item.Key.Contains("injured"))
				continue;

			var option = new Dropdown.OptionData() { text = item.Key };
			dropdownOptions.Add(option);
		}
		speciesDropdown.options = dropdownOptions;

		speciesDropdown.onValueChanged.AddListener(OnDropdownValueChange);
		closeButton.onClick.AddListener(ClosePopup);
	}

	private void ClosePopup()
	{
		gameObject.SetActive(false);
	}

	public void ShowFactFile(string URL)
	{
		if (!initted)
		{
			Init();
		}

		speciesDropdown.gameObject.SetActive(true);

		FactFiles file = AlFactFiles.Find(x => URL.Contains(x.URL));
		if (file == null) //main list popup
		{
			speciesDropdown.value = 1;
			speciesDropdown.value = 0;
		}
		else
		{
			var option = speciesDropdown.options.Find(x => x.text == file.Name);
			speciesDropdown.value = speciesDropdown.options.IndexOf(option);

			speciesDropdown.gameObject.SetActive(false);
		}
	}

	private void OnDropdownValueChange(int newValue)
	{
		var option = speciesDropdown.options[newValue];
		FactFiles file = AlFactFiles.Find(x => x.Name == option.text);
		factText.text = file.FactText;
		factImage.sprite = GetSpriteForImage(file.Image);
	}

	private Sprite GetSpriteForImage(string image)
	{
		var imageData = factfileImages.Find(x => x.imageName == image);
		Sprite sprite = imageData != null ? imageData.sprite : null;
		return sprite;
	}

	public class FactFiles
	{
		public string URL { get; set; }
		public string Name { get; set; }
		public string Image { get; set; }
		public string FactText { get; set; }
	}

	public static List<FactFiles> AlFactFiles = new List<FactFiles>
	{
		new FactFiles() { Name = "Dragonfly",		URL = "plants_insects/dragonfly.shtml",		Image = "dragonfly.jpg",
		FactText ="Dragonfly.\r\nThe biggest difference between modern and fossil dragonflies is that many of the fossilized ones were several times larger, some having wingspans of over three feet! If anything, dragonflies have devolved, not evolved.",
			},
		new FactFiles() { Name = "Centipede",		URL = "plants_insects/centipede.shtml",		Image = "centipede.jpg",
		FactText ="Centipede.\r\nEuphoberia was much like the modern centipede in shape and behavior, but with the distinction of being over three feet long. Fossil accounts of these beasts have been found across Europe and North America.",
			},
		new FactFiles() { Name = "Scorpion",		URL = "plants_insects/scorpion.shtml",		Image = "scorpion.jpg",
		FactText ="Scorpion.\r\nEarly fossil remains are dated about 430 million years ago. Young Scorpions ride on their mothers back for the first weeks of life. The average lifespan in the wild for the Scorpion is 2 to 10 years.",
			},
		new FactFiles() { Name = "Frog",		URL = "bigal/amphibian_reptile/frog.shtml",		Image = "frog.jpg",
		FactText ="Frog.\r\nBeelzebufo ampinga, the so-called 'devil frog,' may be the largest frog that ever lived. These beach-ball-size amphibians, now extinct, grew to 16 inches (41 centimeters) in length and weighed about 10 pounds (4.5 kilograms). The ancient devil frogs may have snatched lizards, small vertebrates, and possibly even hatchling dinosaurs.",
			},
		new FactFiles() { Name = "Lizard",		URL = "bigal/amphibian_reptile/lizard.shtml",		Image = "lizard.jpg",
		FactText ="Lizard.\r\nIt is possible for some lizards to lose their tail when they feel that they are in danger. The tail they leave behind will move and confuse the predator. The tail can grow back but will be slimmer and often a different color.",
			},
		new FactFiles() { Name = "Sphenodontian",		URL = "bigal/amphibian_reptile/tuatara.shtml",		Image = "tuatara.jpg",
		FactText ="Sphenodontian.\r\nSphenodontidae is a family within the reptile group Rhynchocephalia. Most members of this family are only known from fossils but there is one living member, the tuatara from New Zealand. Squamates and sphenodonts both show caudal autotomy (loss of the tail-tip when threatened).",
		},
		new FactFiles() { Name = "Crocodile",		URL = "bigal/amphibian_reptile/deinosuchus.shtml",		Image = "crocodile.jpg",
		FactText ="Crocodile.\r\nThe biggest sea-dwelling crocodile ever found has turned up in the Tunisian desert. The whopper of a prehistoric predator grew to over 30 feet long (nearly ten meters) and weighed three tons. Paleontologists have dubbed the new species Machimosaurus rex.",
			},
		new FactFiles() { Name = "Pterosaur",		URL = "bigal/pterodactylus.shtml",		Image = "pterosaur.gif",
		FactText ="Pterosaur.\r\nThe first pterosaur discovered and described was Pterodactylus Antiquus. It was acquired by a German ruler in the late 1700s and kept in a Wunderkammer, or Curiosity Cabinet; the specimen was eventually named by French naturalist Georges Cuvier, who correctly identified it as a flying reptile.",
			},
		new FactFiles() { Name = "injured Pterosaur",		URL = "bigal/pterodactylus.shtml",		Image = "pterosaur.gif",
		FactText ="Pterosaur.\r\nThe first pterosaur discovered and described was Pterodactylus Antiquus. It was acquired by a German ruler in the late 1700s and kept in a Wunderkammer, or Curiosity Cabinet; the specimen was eventually named by French naturalist Georges Cuvier, who correctly identified it as a flying reptile.",
		},
		new FactFiles() { Name = "Dryosaurus",		URL = "bigal/dryosaurus.shtml",		Image = "dryosaurus.gif",
		FactText ="Dryosaurus.\r\nThey have been estimated to have reached at least 8 to 14 feet long. However, as no known adult specimens of the genus have been found, the adult size remains unknown. Dryosaurus had a horny beak and cheek teeth. Some scientists suggest that it had cheek-like structures to prevent the loss of food while the animal processed it in the mouth.",
		},
		new FactFiles() { Name = "Othnielia",		URL = "bigal/othnielia.shtml",		Image = "othnielia.gif",
		FactText ="Othnielia.\r\nOthnielia was named as a tribute to the paleontologist Othniel C. Marsh. A herbivore, it ate soft, low-lying plants. It was 4ft long and 1ft tall, and weighed about 50 pounds (22.5 kg).",
			},
		new FactFiles() { Name = "Ornitholestes",		URL = "scrub/ornitholestes.shtml",		Image = "ornitholestes.gif",
		FactText ="Ornitholestes.\r\nThere's still a lot of speculation about Ornitholestes: one paleontologist suggests that this dinosaur snatched fish out of lakes and rivers, another maintains that (if Ornitholestes had hunted in packs) it might have been capable of taking down plant-eating dinosaurs as big as Camptosaurus.",
			},
		new FactFiles() { Name = "Stegosaurus",		URL = "scrub/stegosaurus.shtml",		Image = "stegosaurus.gif",
		FactText ="Stegosaurus.\r\nDue to their distinctive combination of broad, upright plates and tail tipped with spikes, Stegosaurus is one of the most recognizable kinds of dinosaur. Today, it is generally agreed that their spikes were most likely used for defense against predators, while their plates may have been used primarily for display.",
			},
		new FactFiles() { Name = "Diplodocus",		URL = "bigal/diplodocus.shtml",		Image = "diplodocus.jpg",
		FactText ="Diplodocus.\r\nDiplodocus had a 26 foot (8 m) long neck and a 45 foot (14 m) long, whip-like tail. Its head was less than 2 feet long and its nostrils were at the top of the head. The front legs were shorter than its back legs, and all legs had elephant-like, five-toed feet.",
			},
		new FactFiles() { Name = "Juvenile Allosaurus",		URL = "scrub/allosaurus.shtml",		Image = "allosaurus.gif",
		FactText ="Allosaurus (juvenile).\r\nAllosaurus is a genus of large theropod dinosaur that lived 155 to 150 million years ago during the late Jurassic period (Kimmeridgian to early Tithonian). The name 'Allosaurus' means 'different lizard'.",
			},
		new FactFiles() { Name = "Male Allosaurus",		URL = "scrub/allosaurus.shtml",		Image = "allosaurus.gif",
		FactText ="Allosaurus (male).\r\nAllosaurus was a large bipedal predator. It had side-facing eyes, and eye crests that may have been used for display. It averaged 8.5 m (28 ft) in length, though fragmentary remains suggest it could have reached over 12 m (39 ft).",
			},
		new FactFiles() { Name = "Female Allosaurus",		URL = "scrub/allosaurus.shtml",		Image = "allosaurus.gif",
		FactText ="Allosaurus (female).\r\nSome paleontologists interpret Allosaurus as having had cooperative social behavior, and hunting in packs, while others believe individuals may have been aggressive toward each other, and that congregations of this genus are the result of lone individuals feeding on the same carcasses.",
			}
	};
}
